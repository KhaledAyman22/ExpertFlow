export type task = {
    description: string
    dueDate: Date,
    id: string
    priority: number,
    status: number,
    title: string
}

export type assignee = {
    id: string,
    name: string
}

export type team = {
    id: string,
    name: string
}

export type user = {
    id: string,
    name: string,
    role: number,
    teams: string[]
}

export type comment = {
    text: string,
    date: Date,
    userName: string
}

export type taskData = {
    id: string,
    title: string,
    description: string,
    dueDate: Date,
    priority: number,
    status: number,
    attachments: Uint8Array,
    assignees: assignee[],
    teams: team[],
    comments: comment[],
    others: other
}

export type other = {
    users: assignee[],
    teams: team[]
}

export type navList = {[page: string]: string}
