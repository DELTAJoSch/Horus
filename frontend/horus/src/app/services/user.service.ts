import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as globals from 'src/app/globals';
import { Login } from '../dtos/loginDto';
import { Observable } from 'rxjs';
import { User } from '../dtos/userDto';

/**
 * Service to facilitate communication with the user endpoint
 */
@Injectable({
  providedIn: 'root'
})
export class UserService {
  private endpoint: string = globals.backend() + 'users/';

  constructor(private http: HttpClient) { }

  /**
   * Log in with the login data
   * @param login The login data
   * @returns Returns ok if logged in, otherwise failure
   */
  login(login: Login): Observable<any> {
    // Note: withCredentials is really important!!! Otherwise set-cookie will be ignored...
    return this.http.post(this.endpoint + 'session', login, {withCredentials: true});
  }

  /**
   * Log out
   * @returns Returns ok if successful
   */
  logout(): Observable<any> {
    return this.http.get(this.endpoint + 'session', {withCredentials: true});
  }

  /**
   * Get the currently logged in user or unauthorized if not logged in
   * @returns Returns the logged in user
   */
  current(): Observable<User> {
    return this.http.get<User>(this.endpoint, {withCredentials: true});
  }
}
