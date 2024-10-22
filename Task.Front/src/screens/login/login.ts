import {url} from "../../common/constants";
import {fetchProtectedResource} from "../../common/httpRequestHelper";

document.getElementById('login-btn')!
    .addEventListener('click', async e => {
        const email = (document.getElementById('email') as HTMLInputElement).value;
        const password = (document.getElementById('password') as HTMLInputElement).value;

        await loginClick(email, password);
    })


async function loginClick(email: string, password: string) {
    await sendLoginRequest(email, password)
    .then(saveTokens)
    .then(getUserData)
    .then(redirectToHomePage)
    .catch((err) => {
        console.error(err);
    });
}

async function sendLoginRequest(email: string, password: string){
    return await fetch(`${url}/identity/login`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({email, password}),
    })
}

async function saveTokens(response: Response) {
    const data = await response.json();
    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
}

async function getUserData() {
    const response = await fetchProtectedResource<{ name: string, role: number }>('GET', '/user/role')
    localStorage.setItem('name', response!.name);
    localStorage.setItem('role', response!.role.toString());
}

async function redirectToHomePage(){
    window.location.assign('../tasks/tasks.html');
}

