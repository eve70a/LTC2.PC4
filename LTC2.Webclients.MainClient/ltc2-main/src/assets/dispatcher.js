function fireOnFileEvent(file) {
    const onFileEvent = new CustomEvent('onFileEvent', {
        detail: { message: file }
      });

    document.dispatchEvent(onFileEvent);
}

window.fireOnFileEvent = fireOnFileEvent;