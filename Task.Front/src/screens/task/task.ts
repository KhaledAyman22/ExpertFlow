import {fetchProtectedResource} from "../../common/httpRequestHelper";
import {taskData} from "../../common/types";
import {navComponent} from "../nav/nav";
import {priority, status} from "../../common/enums";

(async function main() {

    (window as any).downloadAttachment = downloadAttachment;
    (window as any).saveTask = saveTask;
    (window as any).addComment = addComment;

    const params = new URLSearchParams(window.location.search);

    const taskId = params.get('id');

    const taskData = await fetchProtectedResource<taskData>('GET', `/tasks/${taskId}`);

    if (taskData == null)
        return;


    const navBar = document.getElementById('navBar')! as HTMLElement;
    navBar.innerHTML = navComponent();

    renderTask(taskData!);

    function renderTask(taskData: taskData) {
        const taskContainer = document.getElementById('taskContainer')! as HTMLElement;
        
        
        taskContainer.innerHTML = `
            <section class="p-6 bg-white rounded-lg shadow-lg space-y-6">
                <div class="p-4 bg-gray-100 rounded-lg shadow-md">
                    <div class="flex items-center justify-between mb-4">
                        <h1 class="text-3xl font-bold">${taskData.title}</h1>
                       <div>
                            <button onclick="downloadAttachment()" class="w-[13vw] bg-blue-500 hover:bg-blue-600 text-white py-2 px-5 rounded-md">
                                Download Attach.
                            </button>
                            <button onclick="saveTask()" class="w-[13vw] bg-green-500 hover:bg-green-600 text-white py-2 px-5 rounded-md">
                                Save
                            </button>
                        </div>
                    </div>
                    <p class="text-gray-700 mb-2"><strong>Description:</strong> ${taskData.description}</p>
                    <p class="text-gray-700 mb-2"><strong>Due Date:</strong> ${new Date(taskData.dueDate).toISOString().split('T')[0]}</p>
                    <p class="text-gray-700 mb-2"><strong>Priority:</strong> ${priority[taskData.priority]}</p>
                    <p>
                        <span class="text-gray-700 mb-2"><strong>Status:</strong></span> 
                        <input type="radio" id="${`${taskData.id}-${status[status.Todo]}`}" name="status-${taskData.id}" value="${status.Todo}" ${taskData.status === status.Todo ? 'checked' : ''}>
                        <label for="${status[status.Todo]}">${status[status.Todo]}</label>
                        <input type="radio" id="${`${taskData.id}-${status[status.InProgress]}`}" name="status-${taskData.id}" value="${status.InProgress}" ${taskData.status === status.InProgress ? 'checked' : ''}>
                        <label for="${status[status.InProgress]}">${status[status.InProgress]}</label>
                        <input type="radio" id="${`${taskData.id}-${status[status.Completed]}`}" name="status-${taskData.id}" value="${status.Completed}" ${taskData.status === status.Completed ? 'checked' : ''}>
                        <label for="${status[status.Completed]}">${status[status.Completed]}</label>
                    </p>
                </div>
        
                <!-- Assignees Section -->
                <div class="p-4 bg-gray-100 rounded-lg shadow-md">
                    <h2 class="text-2xl font-semibold">Assignees:</h2>
                    <div class="grid grid-cols-4 gap-4">
                        ${taskData.assignees.map(assignee => `
                            <div>
                                <input type="checkbox" id="${assignee.id}" name="assignees" value="${assignee.id}" checked>
                                <label for="${assignee.id}">${assignee.name}</label>
                            </div>
                        `).join('')}
                        ${taskData.others?.users.map(user => `
                            <div>
                                <input type="checkbox" id="${user.id}" name="assignees" value="${user.id}">
                                <label for="assignee-${user.id}">${user.name}</label>
                            </div>
                        `).join('')}
                    </div>
                </div>
        
                <!-- Teams Section -->
                <div class="p-4 bg-gray-100 rounded-lg shadow-md">
                    <h2 class="text-2xl font-semibold">Teams:</h2>
                    <div class="grid grid-cols-4 gap-4">
                        ${taskData.teams.map(team => `
                            <div>
                                <input type="checkbox" id="${team.id}" name="teams" value="${team.id}" checked>
                                <label for="${team.id}">${team.name}</label>
                            </div>
                        `).join('')}
                        ${taskData.others?.teams.map(team => `
                            <div>
                                <input type="checkbox" id="${team.id}" name="teams" value="${team.id}">
                                <label for="${team.id}">${team.name}</label>
                            </div>
                        `).join('')}
                    </div>
                </div>
        
                <div class="p-4 bg-gray-100 rounded-lg shadow-md">
                    <h2 class="text-2xl font-semibold">Comments:</h2>
                    <ul class="space-y-4">
                        ${taskData.comments.map(comment => `
                            <li class="bg-white p-4 rounded-md shadow-md">
                                <strong>${comment.userName}:</strong> ${comment.text} 
                                <span class="text-sm text-gray-500">(${new Date(comment.date).toLocaleDateString()})</span>
                            </li>
                        `).join('')}
                    </ul>
                </div>
        
                <div class="p-4 bg-gray-100 rounded-lg shadow-md">
                    <h2 class="text-2xl font-semibold">Add Comment:</h2>
                    <textarea id="commentInput" class="border rounded-md p-2 w-full h-24 mt-2" placeholder="Type your comment here..."></textarea>
                    <button onclick="addComment()" class="mt-2 bg-green-500 hover:bg-green-600 text-white py-2 px-4 rounded-md">
                        Submit Comment
                    </button>
                </div>
            </section>
        `;

    }

    async function downloadAttachment() {
        try {
            const response = await fetchProtectedResource<Response>('GET',`/tasks/${taskId}/attachments`, null, 'application/octet-stream');

            if (!response.ok) {
                throw new Error('Failed to fetch attachment');
            }

            const blob = await response.blob();
            const url = window.URL.createObjectURL(blob);

            const a = document.createElement('a');
            a.href = url;
            a.download = 'attachment.zip'; // Name the file
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);

            window.URL.revokeObjectURL(url);
        } catch (error) {
            console.error('Error downloading attachment:', error);
        }
    }


    async function addComment() {
        const commentInput = document.getElementById('commentInput') as HTMLInputElement;
        const newCommentText = commentInput.value.trim();

        if (newCommentText) {
            const newComment = {
                text: newCommentText,
                date: new Date().toISOString(),
                taskId: taskId
            };

            await fetchProtectedResource('POST', '/comments', newComment);

            window.location.reload();
        }
    }

    async function saveTask() {
        try {
            let status:string;
            let teams: string[] = [];
            let assignees: string[] = [];

            document.getElementsByName(`status-${taskId}`)
                .forEach((el) => {
                    const input = (el as HTMLInputElement);
                    if (input.checked)
                        status = input.value;
                })

            document.getElementsByName(`teams`)
                .forEach((el) => {
                    const input = (el as HTMLInputElement);
                    if (input.checked)
                        teams.push(input.id)
                })

            document.getElementsByName(`assignees`)
                .forEach((el) => {
                    const input = (el as HTMLInputElement);
                    if (input.checked)
                        assignees.push(input.id)
                })
            const body =  {taskId: taskId, status: Number.parseInt(status!), teams: teams, assignees: assignees};
            const response = await fetchProtectedResource<Response>('PUT', '/tasks', body)

            if (response?.ok) {
                alert(`Task updated successfully!`);
            } else {
                throw new Error(`Error while saving`);
            }

            window.location.assign(`./task.html?id=${taskId}`)
        } catch (error) {
            console.error(error)
        }
    }
})()