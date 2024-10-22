import {url} from "./constants";

export async function fetchProtectedResource<T>(method: string, endpoint: string, body: {} | null = null, contentType: string = 'application/json'): Promise<T> {
    const accessToken = localStorage.getItem('accessToken');

    return await fetch(`${url}${endpoint}`, {
        method: method,
        headers: {
            'Content-Type': contentType,
            'Authorization': `Bearer ${accessToken}`,
        },
        body: body == null ? null : JSON.stringify(body)
    }).then(async (response) => {
        if (response.status == 401) {
            await refreshAccessToken();
            return fetchProtectedResource<T>(method, endpoint, body, contentType);
        }

        if (response.status >= 200 && response.status <= 299) {
            let responseJson;
            try {

                if (contentType == 'application/json')
                    responseJson = await response.json()
                else
                    responseJson = response;
            } catch (e) {
                return response;
            }

            return responseJson;
        }
        return
    }).catch((err) => {
        console.error(err);
        return null;
    });
}


async function refreshAccessToken() {
    const refreshToken = localStorage.getItem('refreshToken');

    const response = await fetch(`${url}/identity/refresh`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({refreshToken}),
    }).then(async (response) => {
        if (response.ok) {
            const data = await response.json();
            localStorage.setItem('accessToken', data.accessToken);
        } else {
            // Handle refresh token failure (e.g., redirect to login)
            localStorage.removeItem('accessToken');
            localStorage.removeItem('refreshToken');
            // Redirect to login or show an error message
        }
    });


}
