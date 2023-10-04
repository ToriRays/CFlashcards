const buttonAnswer = document.querySelectorAll(".speech-button-answer");
let isSpeaking = false;
let speechSynthesis = window.speechSynthesis;
let intervalId;

const textToSpeech = (text, lang) => {
    if (isSpeaking) {
        speechSynthesis.cancel();
        isSpeaking = false;
        buttonAnswer.innerText = "Convert to Speech";
        clearInterval(intervalId);
        return;
    }

    if (text && text.length > 0) {
        const utterance = new SpeechSynthesisUtterance(text);
        utterance.lang = lang; // Set the language of the utterance to Norwegian
        speechSynthesis.speak(utterance);
        isSpeaking = true;
        buttonAnswer.innerText = "Pause";
    }
};

buttonAnswer.forEach((button) => {
    button.addEventListener("click", () => {
        const answerText = button.getAttribute("data-answer");
        // Set the language code for Norwegian (Norway)
        const lang = 'no-NO';
        textToSpeech(answerText, lang);
    });
});

intervalId = setInterval(() => {
    if (!speechSynthesis.speaking && isSpeaking) {
        isSpeaking = false;
        buttonAnswer.innerText = "Convert to Speech";
        clearInterval(intervalId);
    }
});
