// Small helpers the Blazor components call through JS interop.
window.nineTap = (function () {
    let unsavedGuard = false;

    window.addEventListener("beforeunload", function (e) {
        if (unsavedGuard) {
            e.preventDefault();
            e.returnValue = "";
        }
    });

    return {
        // Downloads a stream sent from .NET (DotNetStreamReference) as a file.
        downloadFileFromStream: async function (fileName, contentStreamReference, contentType) {
            const arrayBuffer = await contentStreamReference.arrayBuffer();
            const blob = new Blob([arrayBuffer], { type: contentType || "application/octet-stream" });
            const url = URL.createObjectURL(blob);
            const anchor = document.createElement("a");
            anchor.href = url;
            anchor.download = fileName ?? "";
            document.body.appendChild(anchor);
            anchor.click();
            anchor.remove();
            URL.revokeObjectURL(url);
        },

        // Warn before leaving the page while a form has unsaved edits.
        setUnsavedGuard: function (isDirty) {
            unsavedGuard = !!isDirty;
        },

        // Focus and select an element by id (used after a grid re-render).
        focusById: function (id, selectText) {
            const el = document.getElementById(id);
            if (!el) return;
            el.focus();
            if (selectText && typeof el.select === "function") {
                el.select();
            }
        },

        print: function () {
            window.print();
        }
    };
})();
