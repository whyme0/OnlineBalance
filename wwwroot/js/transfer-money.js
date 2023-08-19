let accountNumInput = document.getElementById("recipAccNum");

document.addEventListener("submit", (e) => {
    newAccNum = accountNumInput.value.replaceAll(" ", "");
    accountNumInput.value = newAccNum;
});