import {fetchProtectedResource} from "../../common/httpRequestHelper";
import {navComponent} from "../nav/nav";
import {userRoles} from "../../common/enums";

(function main() {
        const addUserButton = document.getElementById('addUserButton') as HTMLButtonElement;
        const email = document.getElementById('email') as HTMLInputElement;
        const password = document.getElementById('password') as HTMLInputElement;
        const firstNameInput = document.getElementById('firstName') as HTMLInputElement;
        const lastNameInput = document.getElementById('lastName') as HTMLInputElement;
        const leadMailInput = document.getElementById('leadMail') as HTMLInputElement;
        const userRoleSelect = document.getElementById('role') as HTMLSelectElement;

        Object.entries(userRoles).forEach(([key, value]) => {
            if (typeof value === 'number') {
                const option = document.createElement('option');
                option.value = value.toString();
                option.textContent = key.charAt(0).toUpperCase() + key.slice(1);
                userRoleSelect.appendChild(option);
            }
        });

        const navBar = document.getElementById('navBar')! as HTMLElement;
        navBar.innerHTML = navComponent();

        addUserButton.addEventListener('click', async () => {
            const body = {
                email: email.value.trim(),
                password: password.value.trim(),
                firstName: firstNameInput.value,
                lastName: lastNameInput.value,
                role: Number.parseInt(userRoleSelect.value),
                leadEmail: leadMailInput.value
            }
            const response = await fetchProtectedResource('POST', '/identity/register', body)
            
            if (response == null) {
                alert('Error');
                return;
            }

            email.value = '';
            password.value = '';
            firstNameInput.value = '';
            lastNameInput.value = '';
            leadMailInput.value = '';
            userRoleSelect.value = '';
            alert('User created.');
        });
    }
)()