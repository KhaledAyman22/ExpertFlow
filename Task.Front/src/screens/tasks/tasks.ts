import {fetchProtectedResource} from "../../common/httpRequestHelper";
import {priority, status} from "../../common/enums";
import {navComponent} from "../nav/nav";
import {task} from "../../common/types";

(async function main() {
    (window as any).taskClick = taskClick;
    (window as any).saveTaskStatus = saveTaskStatus;
    
    const tasks = await fetchProtectedResource<task[]>('GET', '/tasks');

    const navBar = document.getElementById('navBar')! as HTMLElement;
    navBar.innerHTML = navComponent();
    
    let header = `
    <section class="bg-blue-100 shadow-md p-6 rounded-lg w-full">
        <h1 class="text-2xl font-bold mb-4">Hello, <span id="userName" class="text-blue-600">${localStorage.getItem('name')}</span></h1>
    </section>
    `;

    document.body.innerHTML += header;
    
    if (tasks == null)
        return;
    
    let tasksHtml = '';
    
    for (const task of tasks) {
        let taskHtml = `
          <div class="border-l-4 border-blue-500 p-4 bg-gray-50 rounded-md w-full  hover:bg-gray-200">
            <h3 class="text-lg font-bold hover:cursor-pointer" onclick="taskClick('${task.id}')">${task.title}</h3>
            <p class="text-gray-700">${task.description}</p>
            <div class="mt-2 text-sm">
                <p><span class="font-semibold">Due Date:</span> ${new Date(task.dueDate).toISOString().split('T')[0]}</p>
                <p><span class="font-semibold">Priority:</span> ${priority[task.priority]}</p>
                <p>
                    <span class="font-semibold">Status:</span> 
                    <input type="radio" id="${`${task.id}-${status[status.Todo]}`}" name="status-${task.id}" value="${status.Todo}" ${task.status === status.Todo ? 'checked' : ''}>
                    <label for="${status[status.Todo]}">${status[status.Todo]}</label>
                    <input type="radio" id="${`${task.id}-${status[status.InProgress]}`}" name="status-${task.id}" value="${status.InProgress}" ${task.status === status.InProgress ? 'checked' : ''}>
                    <label for="${status[status.InProgress]}">${status[status.InProgress]}</label>
                    <input type="radio" id="${`${task.id}-${status[status.Completed]}`}" name="status-${task.id}" value="${status.Completed}" ${task.status === status.Completed ? 'checked' : ''}>
                    <label for="${status[status.Completed]}">${status[status.Completed]}</label>
                </p>
                <button id="${`${task.id}-save`}" class="save-btn mt-4 px-4 py-2 bg-green-500 text-white rounded hover:bg-green-600" onclick="saveTaskStatus('${task.id}')">
                    Save
                </button>
            </div>
          </div>
        `;
        
        tasksHtml += taskHtml;
    }

    const taskList = `
    <section class="bg-green-100 shadow-md p-6 rounded-lg w-full mt-6 flex-1 overflow-auto">
        <h2 class="text-xl font-semibold mb-4">Your Tasks</h2>
    
        <div id="taskList" class="space-y-4">
          ${tasksHtml}
        </div>
    </section>
    `

    document.body.innerHTML += taskList;
})()

function taskClick(id: string) {
    window.location.assign(`../task/task.html?id=${id}`);
}

async function saveTaskStatus(taskId: string) {
    try {
        let val:string;
        document.getElementsByName(`status-${taskId}`)
            .forEach((el) => {
                const input = (el as HTMLInputElement);
                if (input.checked)
                    val = input.value;
            })
        const body =  {taskId: taskId, status: Number.parseInt(val!)};
        const response = await fetchProtectedResource<Response>('PUT', '/tasks', body)

        if (response?.ok) {
            alert(`Task updated successfully!`);
        } else {
            throw new Error(`Error while saving`);
        }

        window.location.assign(`./tasks.html`)
    } catch (error) {
        console.error(error)
    }
}
