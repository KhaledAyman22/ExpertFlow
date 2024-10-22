import {navComponent} from "../nav/nav";
import {fetchProtectedResource} from "../../common/httpRequestHelper";
import {team, user} from "../../common/types";
import {userRoles} from "../../common/enums";

let currentPage = 1;
const usersPerPage = 10;

const params = new URLSearchParams(window.location.search);
const page = params.get('page');

(
    async function main() {
        const navBar = document.getElementById('navBar')! as HTMLElement;
        navBar.innerHTML = navComponent();

        const pageData = await loadData();
        renderUsers();
        setupPagination();
        setupEventListeners();


        async function loadData() {
            const teams = await fetchProtectedResource<{ teams: team[] }>('GET', '/teams')
            const users = await fetchProtectedResource<{ users: user[] }>('GET', `/users/${page}`)

            return {
                users: users?.users,
                teams: teams?.teams 
            };
        }

        function renderUsers() {
            const userList = document.getElementById('user-list');
            if (!userList) return;

            userList.innerHTML = '';

            const startIndex = (currentPage - 1) * usersPerPage;
            const endIndex = startIndex + usersPerPage;
            const currentUsers = pageData.users!.slice(startIndex, endIndex);

            currentUsers.forEach(user => {
                const userElement = document.createElement('div');
                userElement.classList.add('p-4', 'bg-white', 'mb-4', 'rounded', 'shadow');

                userElement.innerHTML = `
            <h2 class="text-xl font-semibold">${user.name}</h2>
            <p class="text-gray-500">Role: ${userRoles[user.role]}</p>
            <div class="mt-2">
                <h3 class="text-lg font-medium">Assign to Teams:</h3>
                <div id="team-checkboxes-${user.id}" class="mt-2">
                    ${renderTeamCheckboxes(user)}
                </div>
                <button data-user-id="${user.id}" class="save-btn mt-4 px-4 py-2 bg-green-500 text-white rounded hover:bg-green-600">
                    Save
                </button>
            </div>
        `;

                userList.appendChild(userElement);
            });

            document.getElementById('page-indicator')!.textContent = `Page ${currentPage}`;
        }

        function renderTeamCheckboxes(user: user): string {
            return `
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
            ${pageData.teams!.map(team => {
                const isChecked = user.teams.includes(team.id) ? 'checked' : '';
                return `
                    <label class="block">
                        <input type="checkbox" data-user-id="${user.id}" data-team-id="${team.id}" ${isChecked} class="team-checkbox">
                        ${team.name}
                    </label>
                `;
            }).join('')}
        </div>
    `;
        }

        function handleCheckboxChange(event: Event) {
            const target = event.target as HTMLInputElement;
            const userId = target.dataset.userId!;
            const teamId = target.dataset.teamId!;

            const user = pageData.users!.find(u => u.id === userId);
            if (!user) return;

            if (target.checked) {
                if (!user.teams.includes(teamId)) {
                    user.teams.push(teamId);
                }
            } else {
                user.teams = user.teams.filter(id => id !== teamId);
            }
        }

        async function saveUserData(userId: string) {
            const user = pageData.users!.find(u => u.id === userId);
            if (!user) return;

            try {
                const body =  {userId: user.id, teams: user.teams};
                const response = await fetchProtectedResource<Response>('PATCH', '/users/assign', body)

                if (response?.ok) {
                    alert(`User ${user.name}'s team assignments saved successfully!`);
                } else {
                    throw new Error(`Error saving user ${user.name}'s data`);
                }
                
                window.location.assign(`./userManagement.html?page=${page}`)
            } catch (error) {
                console.error(error)
            }
        }

        function setupPagination() {
            const prevButton = document.getElementById('prev-page');
            const nextButton = document.getElementById('next-page');

            prevButton?.addEventListener('click', () => {
                if (currentPage > 1) {
                    currentPage--;
                    renderUsers();
                }
            });

            nextButton?.addEventListener('click', () => {
                if (currentPage * usersPerPage < pageData.users!.length) {
                    currentPage++;
                    renderUsers();
                }
            });
        }

        function setupEventListeners() {
            document.addEventListener('change', event => {
                if ((event.target as Element).classList.contains('team-checkbox')) {
                    handleCheckboxChange(event);
                }
            });

            document.addEventListener('click', event => {
                const target = event.target as HTMLButtonElement;
                if (target.classList.contains('save-btn')) {
                    const userId = target.dataset.userId!;
                    saveUserData(userId);
                }
            });
        }
    }
)()