let inputFieldId = document.currentScript.getAttribute("inputElementId");
let outputFieldId = document.currentScript.getAttribute("outputElementId");
let maxlen = parseInt(document.currentScript.getAttribute("maxlength"), 10);

let inputField = document.getElementById(inputFieldId);
let outputField = document.getElementById(outputFieldId);

if (inputField == null) {
    console.error("Input field not found! Check 'inputElementId' attribute!")
}
if (outputField == null) {
    console.error("Input field not found! Check 'outputElementId' attribute!")
}

inputField.addEventListener("keyup", (event) => {
    let len = inputField.value.length;
    let message = `You're entered ${len} of ${maxlen} chars`;
    outputField.innerText = message;
});