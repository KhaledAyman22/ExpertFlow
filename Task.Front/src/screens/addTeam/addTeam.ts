import {fetchProtectedResource} from "../../common/httpRequestHelper";
import {navComponent} from "../nav/nav";

(function main(){
        const addTeamButton = document.getElementById('addTeamButton') as HTMLButtonElement;
        const teamNameInput = document.getElementById('teamName') as HTMLInputElement;

        const navBar = document.getElementById('navBar')! as HTMLElement;
        navBar.innerHTML = navComponent();
        
        addTeamButton.addEventListener('click', async() => {
            const teamName = teamNameInput.value.trim();
            if (teamName) {
                const response = await fetchProtectedResource('POST','/teams', { name: teamName })
                if (response == null){
                    alert('Error');
                    return;
                }
                teamNameInput.value = '';
                alert('Team created.');
            } else {
                alert('Please enter a team name.');
            }
        });
    }
)()