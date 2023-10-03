const textarea = document.querySelector("textarea");
const buttonQuestion = document.querySelector(".speech-button");
const buttonAnswer = document.querySelectorAll(".speech-button-answer");
let isSpeaking = false;
let speechSynthesis = window.speechSynthesis;
let intervalId;

const textToSpeech = (text, lang) => {
    if (isSpeaking) {
        speechSynthesis.cancel();
        isSpeaking = false;
        buttonQuestion.innerText = "Convert to Speech";
        clearInterval(intervalId);
        return;
    }

    if (text && text.length > 0) {
        const utterance = new SpeechSynthesisUtterance(text);
        utterance.lang = lang; // Set the language of the utterance to Norwegian
        speechSynthesis.speak(utterance);
        isSpeaking = true;
        buttonQuestion.innerText = "Pause";
    }
};

buttonQuestion.addEventListener("click", () => {
    const text = textarea.value;
    // Set the language code for Norwegian (Norway)
    const lang = 'no-NO';
    textToSpeech(text, lang);
});

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
        buttonQuestion.innerText = "Convert to Speech";
        clearInterval(intervalId);
    }
});
