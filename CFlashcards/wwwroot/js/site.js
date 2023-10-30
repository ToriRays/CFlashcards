const buttonAnswer = document.querySelectorAll(".speech-button-answer");
let isSpeaking = false;
let speechSynthesis = window.speechSynthesis;
let intervalId;

const textToSpeech = (button, text, lang) => {
if (isSpeaking) {
    speechSynthesis.cancel();
    isSpeaking = false;
    clearInterval(intervalId);
    return;
}

if (text && text.length > 0) {
    const utterance = new SpeechSynthesisUtterance(text);
    utterance.lang = lang; // Set the language of the utterance to English
    speechSynthesis.speak(utterance);
    isSpeaking = true;
    buttonAnswer.innerText = "Pause";
}
};

buttonAnswer.forEach((button) => {
button.addEventListener("click", () => {
    const answerText = button.getAttribute("data-answer");
    // Set the language code for Norwegian 
    const lang = 'no-No';
    textToSpeech(button, answerText, lang);
});
});

intervalId = setInterval(() => {
    if (!speechSynthesis.speaking && isSpeaking) {
        isSpeaking = false;
        buttonAnswer.forEach((button) => {
        });
        clearInterval(intervalId);
    }
});




// Initialize the KUTE.js morph animation between blob1 and blob2

const morphing = KUTE.fromTo(
'#blob1',
{ path: '#blob1' },
{ path: '#blob2' },
{ duration: 2000, easing: 'easingQuadraticOut' }
);

// Start the animation
morphing.start();

