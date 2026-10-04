(() => {
  const bridge = window.chrome?.webview;
  const send = message => bridge?.postMessage(message);
  const terminal = new Terminal({
    fontFamily: 'Cascadia Mono, Consolas, Malgun Gothic, monospace',
    fontSize: 14,
    lineHeight: 1.28,
    cursorBlink: true,
    cursorStyle: 'bar',
    scrollback: 6000,
    convertEol: false,
    theme: { background: '#0d1117', foreground: '#dce4ee', cursor: '#82e9b0', selectionBackground: '#506d84aa', black: '#222c37', red: '#f27e7e', green: '#80e1a2', yellow: '#e6c079', blue: '#8cb6f4', magenta: '#bf9afa', cyan: '#81d9e9', white: '#dce4ee' }
  });
  const fit = new FitAddon.FitAddon();
  terminal.loadAddon(fit);
  terminal.open(document.getElementById('terminal'));
  const modal = document.getElementById('composer-backdrop');
  const textarea = document.getElementById('script');
  const status = document.getElementById('status');
  let ready = false;

  function resize() {
    fit.fit();
    send({ type: 'resize', cols: terminal.cols, rows: terminal.rows });
  }
  new ResizeObserver(resize).observe(document.getElementById('terminal-wrap'));
  terminal.onData(data => { if (ready) send({ type: 'input', data }); });

  function openComposer(content = '') {
    textarea.value = content;
    modal.classList.remove('hidden');
    modal.setAttribute('aria-hidden', 'false');
    textarea.focus();
    textarea.setSelectionRange(textarea.value.length, textarea.value.length);
  }
  function closeComposer() {
    modal.classList.add('hidden');
    modal.setAttribute('aria-hidden', 'true');
    terminal.focus();
  }
  function paste(text) {
    if (!text) return;
    const normalized = text.replace(/\r\n?/g, '\n');
    if (normalized.includes('\n')) openComposer(normalized);
    else if (ready) send({ type: 'input', data: '\x1b[200~' + normalized + '\x1b[201~' });
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
    if (!modal.classList.contains('hidden')) return;
    event.preventDefault();
    paste(event.clipboardData?.getData('text/plain') || '');
  });
  document.addEventListener('keydown', event => {
    if (event.key === 'Escape' && !modal.classList.contains('hidden')) closeComposer();
    if (event.ctrlKey && event.shiftKey && event.key.toLowerCase() === 'E' && modal.classList.contains('hidden')) {
      event.preventDefault(); openComposer();
    }
  });
  textarea.addEventListener('keydown', event => {
    if (event.ctrlKey && event.key === 'Enter') { event.preventDefault(); run(); }
    if (event.key === 'Tab') { event.preventDefault(); textarea.setRangeText('    ', textarea.selectionStart, textarea.selectionEnd, 'end'); }
  });
  function run() {
    const value = textarea.value;
    if (!value.trim() || !ready) return;
    send({ type: 'run', data: value });
    closeComposer();
  }
  document.getElementById('compose').onclick = () => openComposer();
  document.getElementById('close-composer').onclick = closeComposer;
  document.getElementById('run').onclick = run;
  document.getElementById('paste').onclick = () => send({ type: 'clipboard-read' });
  document.getElementById('copy').onclick = () => { if (terminal.hasSelection()) send({ type: 'clipboard-write', data: terminal.getSelection() }); terminal.focus(); };
  document.getElementById('clear').onclick = () => { terminal.clear(); terminal.focus(); };

  bridge?.addEventListener('message', event => {
    const message = event.data;
    switch (message.type) {
      case 'output': terminal.write(message.data); break;
      case 'ready':
        ready = true;
        status.classList.add('ready');
        status.textContent = '● Git Bash 연결됨';
        resize(); terminal.focus(); break;
      case 'clipboard': paste(message.data); break;
      case 'fatal':
        status.classList.add('error'); status.textContent = '● 시작 실패';
        terminal.writeln('\r\n\x1b[31m' + message.message + '\x1b[0m'); break;
      case 'exited':
        ready = false; status.classList.remove('ready');
        status.textContent = '● 셸 종료됨'; break;
      case 'error': terminal.writeln('\r\n\x1b[31m' + message.message + '\x1b[0m'); break;
    }
  });
  resize();
})();
