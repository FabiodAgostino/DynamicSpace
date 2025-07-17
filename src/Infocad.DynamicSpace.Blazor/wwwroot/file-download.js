window.downloadFile = (fileName, contentType, content) => {
    const bytes = new Uint8Array(content);
    const blob = new Blob([bytes], { type: contentType });
    const url = window.URL.createObjectURL(blob);

    const link = document.createElement('a');
    link.href = url;
    link.download = fileName || 'download';
    link.click();

    link.remove();
    window.URL.revokeObjectURL(url);
};