const textarea = document.querySelector("textarea");
const button = document.querySelector(".speech-button");
let isSpeaking = false;
let speechSynthesis = window.speechSynthesis;
let intervalId;

const textToSpeech = () => {
    const text = textarea.value;

    if (isSpeaking) {
        speechSynthesis.cancel();
        isSpeaking = false;
        button.innerText = "Convert to Speech";
        clearInterval(intervalId);
        return;
    }

    if (text && text.length > 0) {
        const utterance = new SpeechSynthesisUtterance(text);
        speechSynthesis.speak(utterance);
        isSpeaking = true;
        button.innerText = "Pause";
    }
}; 

button.addEventListener("click", textToSpeech);

intervalId = setInterval(() => {
    if (!speechSynthesis.speaking && isSpeaking) {
        isSpeaking = false;
        button.innerText = "Convert to Speech";
        clearInterval(intervalId);
    }
}); // Adjust the interval duration as needed
