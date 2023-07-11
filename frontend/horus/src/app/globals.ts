import { environment } from "src/environments/environment";
import { User } from "./dtos/userDto";
'use strict';

/**
 * Get the url for backend calls
 * @returns Returns the backend URL
 */
export function backend(): string {
    return environment.backend + apiVersion;
}

export const apiVersion = 'v1/';

export let user : User | undefined;
export let loggedIn : boolean = false;

export function setUser(user: User | undefined) {
    loggedIn = user !== undefined;
    user = user;
}