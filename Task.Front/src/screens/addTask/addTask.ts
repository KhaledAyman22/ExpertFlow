import {fetchProtectedResource} from "../../common/httpRequestHelper";
import {navComponent} from "../nav/nav";

(window as any).addTask = addTask;

const navBar = document.getElementById('navBar')! as HTMLElement;
navBar.innerHTML = navComponent();


async function addTask() {
    const title = (document.getElementById('title') as HTMLInputElement).value;
    const description = (document.getElementById('description') as HTMLInputElement).value;
    const dueDate = new Date((document.getElementById('dueDate') as HTMLInputElement).value).toISOString();
    const priority = (document.getElementById('priority') as HTMLInputElement).value;
    const status = (document.querySelector('input[name="status"]:checked') as HTMLInputElement).value;
    const attachments = (document.getElementById('attachment') as HTMLInputElement).files!;

    let attachmentsBase64:string[] = [];
    
    if (attachments.length > 0) {
        attachmentsBase64 = await Promise.all<string>([...attachments].map(file => {
            return new Promise((resolve) => {
                const reader = new FileReader();
                reader.onload = () => {
                    const base64String = (reader.result! as string).split(',')[1];
                    resolve(base64String);
                };
                reader.readAsDataURL(file);
            });
        }));
    }


    // Create the request body
    const requestBody = {
        title,
        description,
        dueDate,
        priority: Number.parseInt(priority),
        status: Number.parseInt(status),
        attachments: attachmentsBase64
    };

    // Send the request to the backend
    try {
        
        const response = await fetchProtectedResource<Response>('POST', '/tasks', requestBody)

        if (response.ok) {
            alert('Task added successfully!');
            (document.getElementById('taskForm') as HTMLFormElement).reset();
        } else {
            const error = await response.json();
            alert(`Error adding task: ${error.message}`);
        }
    } catch (error) {
        console.error('Error:', error);
        alert('An error occurred while adding the task. Please try again.');
    }
}