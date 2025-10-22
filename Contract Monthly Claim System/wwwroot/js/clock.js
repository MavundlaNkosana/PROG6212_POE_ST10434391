let clocking = false;
let startTime = null;
const btn = document.getElementById('clockBtn');

btn.addEventListener('click', async () => {
    if (!clocking) {
        startTime = new Date();
        clocking = true;
        btn.innerText = 'Clock Out';
    } else {
        const endTime = new Date();
        clocking = false;
        btn.innerText = 'Clock In / Out';
        const claimId = document.querySelector('input[name="id"]').value;
        const payload = new URLSearchParams();
        payload.append('id', claimId);
        payload.append('start', startTime.toISOString());
        payload.append('end', endTime.toISOString());
        await fetch('/Claims/ClockIn', { method: 'POST', body: payload })
            .then(r => r.json())
            .then(j => { document.getElementById('lastSession').innerText = `Added ${j.hours.toFixed(2)} hrs, amount ${j.amount}`; })
            .catch(e => console.error(e));
    }
});
