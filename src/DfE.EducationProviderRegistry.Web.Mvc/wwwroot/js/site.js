document.addEventListener("DOMContentLoaded", () => {
    document
        .querySelectorAll("[data-module='app-autocomplete']")
        .forEach(select => {
            accessibleAutocomplete.enhanceSelectElement({
                selectElement: select,
                showAllValues: true,
                default: "",
                autoselect: false,
                placeholder: "Search local authorities"
            });

            const autocompleteInput =
                select.parentElement.querySelector(".autocomplete__input");

            autocompleteInput?.addEventListener("input", event => {
                if (event.target.value.trim() === "") {
                    select.value = "";
                }
            });
        });

    const sortSelect = document.getElementById("sort");

    if (sortSelect instanceof HTMLSelectElement) {
        sortSelect.addEventListener("change", () => {
            sortSelect.form?.submit();
        });
    }
});
