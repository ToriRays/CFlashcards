// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Initialize the KUTE.js morph animation between blob1 and blob2
const tween = KUTE.fromTo(
    '#blob1',
    { path: '#blob1' },
    { path: '#blob2' },
    { duration: 2000, easing: 'easingQuadraticOut' }
);

// Start the animation
tween.start();
