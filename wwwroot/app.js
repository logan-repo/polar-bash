(() => {
  const bridge = window.chrome?.webview;
  const send = message => bridge?.postMessage(message);
  const byId = id => document.getElementById(id);
  const composer = byId('composer-backdrop');
  const settings = byId('settings-backdrop');
  const textarea = byId('script');
  const status = byId('status');
  const statusLabel = byId('status-label');
  const fontSize = byId('font-size');
  let ready = false;
  let theme = localStorage.getItem('polar-theme') === 'light' ? 'light' : 'dark';

  const palettes = {
    dark: {
      background: '#012c4e', foreground: '#e7f3f5', cursor: '#ffe8a9',
      selectionBackground: '#8cc5db66', black: '#142f42', red: '#ee808a',
      green: '#66dfae', yellow: '#f6d98b', blue: '#98c9ff',
      magenta: '#c9a9fc', cyan: '#72e4e5', white: '#e7f3f5'
    },
    light: {
      background: '#fffdf8', foreground: '#173b56', cursor: '#023859',
      selectionBackground: '#dfc67999', black: '#173b56', red: '#ac3441',
      green: '#087357', yellow: '#a76c17', blue: '#245aa8',
      magenta: '#8143a8', cyan: '#0a7a8c', white: '#f9f3e8'
    }
  };

  const savedSize = Number(localStorage.getItem('polar-font-size'));
  const initialSize = Number.isInteger(savedSize) && savedSize >= 12 && savedSize <= 22 ? savedSize : 14;
  const terminal = new Terminal({
    fontFamily: '"Cascadia Mono", Consolas, "Malgun Gothic", monospace',
    fontSize: initialSize,
    lineHeight: 1.25,
    cursorBlink: true,
    cursorStyle: 'bar',
    scrollback: 8000,
    convertEol: false,
    theme: palettes[theme]
  });
  const fit = new FitAddon.FitAddon();
  terminal.loadAddon(fit);
  terminal.open(byId('terminal'));
  fontSize.value = String(initialSize);
  byId('font-size-value').textContent = initialSize + 'px';

  function resize() {
    fit.fit();
    send({ type: 'resize', cols: terminal.cols, rows: terminal.rows });
  }
  new ResizeObserver(resize).observe(byId('terminal-wrap'));
  terminal.onData(data => { if (ready) send({ type: 'input', data }); });

  function setTheme(value) {
    theme = value === 'light' ? 'light' : 'dark';
    document.documentElement.dataset.theme = theme;
    terminal.options.theme = palettes[theme];
    localStorage.setItem('polar-theme', theme);
    for (const value of ['dark', 'light']) {
      byId('theme-' + value).classList.toggle('selected', value === theme);
      byId('theme-' + value).setAttribute('aria-pressed', String(value === theme));
    }
    resize();
  }
  setTheme(theme);

  function show(dialog, focusTarget) {
    dialog.classList.remove('hidden');
    dialog.setAttribute('aria-hidden', 'false');
    focusTarget.focus();
  }
  function hide(dialog) {
    dialog.classList.add('hidden');
    dialog.setAttribute('aria-hidden', 'true');
    terminal.focus();
  }
  function openComposer(content = '') {
    textarea.value = content;
    show(composer, textarea);
    textarea.setSelectionRange(textarea.value.length, textarea.value.length);
  }
  function paste(text) {
    if (!text) return;
    const normalized = text.replace(/\r\n?/g, '\n');
    if (normalized.includes('\n')) openComposer(normalized);
    else if (ready) send({ type: 'input', data: '\x1b[200~' + normalized + '\x1b[201~' });
  }
  function run() {
    const value = textarea.value;
    if (!value.trim() || !ready) return;
    send({ type: 'run', data: value });
    hide(composer);
  }

  terminal.attachCustomKeyEventHandler(event => {
    if (event.type !== 'keydown') return true;
    if (event.ctrlKey && !event.altKey && event.key.toLowerCase() === 'v') {
      event.preventDefault(); send({ type: 'clipboard-read' }); return false;
    }
    if (event.ctrlKey && !event.altKey && event.key.toLowerCase() === 'c') {
      event.preventDefault();
      if (terminal.hasSelection()) { send({ type: 'clipboard-write', data: terminal.getSelection() }); terminal.clearSelection(); }
      else if (ready) send({ type: 'input', data: '\x03' });
      return false;
    }
    if (event.ctrlKey && event.shiftKey && event.key.toLowerCase() === 'e') {
      event.preventDefault(); openComposer(); return false;
    }
    return true;
  });

  document.addEventListener('paste', event => {
    if (!composer.classList.contains('hidden') || !settings.classList.contains('hidden')) return;
    event.preventDefault();
    paste(event.clipboardData?.getData('text/plain') || '');
  });
  document.addEventListener('keydown', event => {
    if (event.key === 'Escape') {
      if (!composer.classList.contains('hidden')) hide(composer);
      else if (!settings.classList.contains('hidden')) hide(settings);
    }
    if (event.ctrlKey && event.shiftKey && event.key.toLowerCase() === 'e' && composer.classList.contains('hidden')) {
      event.preventDefault(); openComposer();
    }
  });
  textarea.addEventListener('keydown', event => {
    if (event.ctrlKey && event.key === 'Enter') { event.preventDefault(); run(); }
    if (event.key === 'Tab') { event.preventDefault(); textarea.setRangeText('    ', textarea.selectionStart, textarea.selectionEnd, 'end'); }
  });

  byId('compose').onclick = () => openComposer();
  byId('close-composer').onclick = () => hide(composer);
  byId('run').onclick = run;
  byId('paste').onclick = () => send({ type: 'clipboard-read' });
  byId('copy').onclick = () => { if (terminal.hasSelection()) send({ type: 'clipboard-write', data: terminal.getSelection() }); terminal.focus(); };
  byId('focus-terminal').onclick = () => terminal.focus();
  byId('open-folder').onclick = () => send({ type: 'open-folder' });
  byId('theme-toggle').onclick = () => setTheme(theme === 'dark' ? 'light' : 'dark');
  byId('settings').onclick = () => show(settings, byId('theme-' + theme));
  byId('close-settings').onclick = () => hide(settings);
  byId('done-settings').onclick = () => hide(settings);
  byId('theme-dark').onclick = () => setTheme('dark');
  byId('theme-light').onclick = () => setTheme('light');
  fontSize.oninput = () => {
    const value = Number(fontSize.value);
    terminal.options.fontSize = value;
    byId('font-size-value').textContent = value + 'px';
    localStorage.setItem('polar-font-size', String(value));
    resize();
  };

  bridge?.addEventListener('message', event => {
    const message = event.data;
    switch (message.type) {
      case 'output': terminal.write(message.data); break;
      case 'ready':
        ready = true;
        byId('start-path').textContent = message.workingDirectory;
        byId('start-path').title = message.workingDirectory;
        status.classList.add('ready');
        statusLabel.textContent = '연결됨';
        resize(); terminal.focus(); break;
      case 'clipboard': paste(message.data); break;
      case 'fatal':
        status.classList.add('error'); statusLabel.textContent = '시작 실패';
        terminal.writeln('\r\n\x1b[31m' + message.message + '\x1b[0m'); break;
      case 'exited':
        ready = false; status.classList.remove('ready');
        statusLabel.textContent = '셸 종료됨'; break;
      case 'error': terminal.writeln('\r\n\x1b[31m' + message.message + '\x1b[0m'); break;
    }
  });
  resize();
})();
