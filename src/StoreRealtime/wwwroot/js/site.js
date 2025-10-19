window.scrollToBottom = (element) => {
    try {
        // When called from Blazor with an ElementReference, 'element' will be the DOM node.
        if (!element) return;

        // If it's an HTMLElement, scroll its content to the bottom smoothly
        if (typeof element.scrollTo === 'function') {
            // Use requestAnimationFrame to ensure DOM updated before scrolling
            requestAnimationFrame(() => {
                try {
                    element.scrollTo({ top: element.scrollHeight, behavior: 'smooth' });
                }
                catch (e) {
                    // Fallback
                    element.scrollTop = element.scrollHeight;
                }
            });
            return;
        }

        // Fallback: if a string id was passed, try to resolve it
        if (typeof element === 'string') {
            var el = document.getElementById(element);
            if (el) el.scrollTop = el.scrollHeight;
        }
    }
    catch (err) {
        console.error('scrollToBottom error', err);
    }
};

