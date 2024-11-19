document.addEventListener("DOMContentLoaded", () => {
    let installPrompt = null;
    const installButton = document.querySelector("#installPWA");
    const underLogoDiv = document.querySelector("#collapseLogo");

    if (!installButton || !underLogoDiv) {
        console.warn("Install button or under logo div not found in the DOM.");
        return;
    }

    // beforeinstallprompt is triggered when the browser determines the app meets the PWA installation criteria.
    window.addEventListener("beforeinstallprompt", (event) => {
        event.preventDefault();
        installPrompt = event;
        underLogoDiv.removeAttribute("hidden");
    });

    installButton.addEventListener("click", async () => {
        if (!installPrompt) return;
        const result = await installPrompt.prompt();
        console.log(`Install prompt was: ${result.outcome}`);
        disableInAppInstallPrompt();
    });

    function disableInAppInstallPrompt() {
        installPrompt = null;
        underLogoDiv.setAttribute("hidden", "");
    }

    window.addEventListener("appinstalled", () => {
        disableInAppInstallPrompt();
    });
});