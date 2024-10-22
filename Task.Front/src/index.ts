import './style.css';

const app = document.getElementById('app');
if (app) {
    // const button = Button('Click me', () => alert('Button clicked!'));
    // app.appendChild(button);

    const role = localStorage.getItem('role');
    
    if (role == null) {
        window.location.assign('screens/login/login.html');
    }
    else{
        window.location.assign('screens/tasks/tasks.html');
    }
}
