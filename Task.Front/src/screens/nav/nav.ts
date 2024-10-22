import {navList} from "../../common/types";
import {userRoles} from "../../common/enums";

export function navComponent() {

    const navList = getNavPages();
    

    
    (window as any).logout = logout;
    
    let links : string[] = [];
    for (const page in navList) {
        links.push(`<a href="${navList[page]}" class="text-white hover:text-gray-200">${page}</a>\n`);
    }

    return `
    <nav class="bg-blue-500 p-4 w-screen">
        <div class="container mx-auto flex justify-between items-center">
            <!-- Menu Items -->
            <div class="hidden md:flex space-x-6">
               ${links.join('')}
            </div>

            <div>
                <button onclick="logout()" class="bg-red-500 hover:bg-red-600 text-white py-2 px-4 rounded-md">
                    Logout
                </button>
            </div>
        </div>
    </nav>
    `;
} 


function getNavPages() : navList{
    const role = Number.parseInt(localStorage.getItem('role')!);
    
    if (userRoles.administrator == role){
        return {
            'Add Task': '../addTask/addTask.html',
            'Tasks': '../tasks/tasks.html',
            'Add Team': '../addTeam/addTeam.html',
            'Add User': '../addUser/addUser.html',
            'Team Management': '../userManagement/userManagement.html?page=1',
        }
    }

    else{
        return {
            'Add Task': '../addTask/addTask.html',
            'Tasks': '../tasks/tasks.html',
        }
    }
    
    return {};
}

function logout() {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    window.location.href = '../login/login.html';
}