// Purpose: TypeScript file for the index.html html-page 

// Theme selection
const htmlPageElement = document.getElementById('html-page');
const darkModeElement = document.getElementById('dark-mode');
const lightModeElement = document.getElementById('light-mode');

darkModeElement?.addEventListener('click', () => {
    console.log('Dark mode selected');
    htmlPageElement?.setAttribute('data-bs-theme', 'dark');
});

lightModeElement?.addEventListener('click', () => {
    console.log('Light mode selected');
    htmlPageElement?.setAttribute('data-bs-theme', 'light');
});

// Run prism after htmx request
document.addEventListener('htmx:afterRequest', function(event) {
  console.log('HTMX afterRequest event triggered');
  Prism.highlightAll();
});
