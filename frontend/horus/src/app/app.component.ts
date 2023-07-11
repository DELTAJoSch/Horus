import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';
import * as globals from 'src/app/globals';
import { UserService } from './services/user.service';
import { Router } from '@angular/router';
import { ErrorService } from './services/error.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  constructor(
    private userService: UserService,
    private router: Router,
    private error: ErrorService
  ) {
    if(!environment.production){
      console.log('Dev Build. Do not use in Production!');
    }

    userService.current().subscribe({
      next: current => {
        globals.setUser(current);
        router.navigateByUrl('/');
      },
      error: e => {
        error.handle(e);
      }
    });
  }

  title = environment.title;
}
