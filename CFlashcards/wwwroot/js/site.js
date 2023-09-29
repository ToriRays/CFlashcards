const textarea = document.querySelector("textarea");
const buttonQuestion = document.querySelector(".speech-button");
const buttonAnswer = document.querySelectorAll(".speech-button-answer");
let isSpeaking = false;
let speechSynthesis = window.speechSynthesis;
let intervalId;

const textToSpeech = (text) => {
    if (isSpeaking) {
        speechSynthesis.cancel();
        isSpeaking = false;
        buttonQuestion.innerText = "Convert to Speech";
        clearInterval(intervalId);
        return;
    }

    if (text && text.length > 0) {
        const utterance = new SpeechSynthesisUtterance(text);
        speechSynthesis.speak(utterance);
        isSpeaking = true;
        buttonQuestion.innerText = "Pause";
    }
};

buttonQuestion.addEventListener("click", () => {
    textToSpeech(textarea.value);
});

buttonAnswer.forEach((button) => {
    button.addEventListener("click", () => {
        const answerText = button.getAttribute("data-answer");
        textToSpeech(answerText);
    });
});

intervalId = setInterval(() => {
    if (!speechSynthesis.speaking && isSpeaking) {
        isSpeaking = false;
        buttonQuestion.innerText = "Convert to Speech";
        clearInterval(intervalId);
    }
});
