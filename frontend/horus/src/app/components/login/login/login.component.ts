import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { Login } from 'src/app/dtos/loginDto';
import { ErrorService } from 'src/app/services/error.service';
import { UserService } from 'src/app/services/user.service';
import * as globals from 'src/app/globals';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  glasses = false;
  motd: string = '';

  loginForm = new FormGroup({
    username: new FormControl(''),
    password: new FormControl('')
  });

  constructor(private user: UserService,
    private error: ErrorService,
    private router: Router) {

  }

  /**
   * Initializes this component
   */
  ngOnInit() {
    let num = Math.floor(Math.random() * 50);
    if(num === 10) {
      this.glasses = true;
      this.motd = 'I\'m watching you!';
      return;
    }

    num = Math.floor(Math.random() * 12);
    switch (num) {
      case 0:
        this.motd = 'Consulting magic mirror...';
        break;
      case 1:
        this.motd = '418 I\'m a teapot';
        break;
      case 2:
        this.motd = 'Requesting Server Crashes';
        break;
      case 3:
        this.motd = 'Kernel Panic! at the Disco';
        break;
      case 4:
        this.motd = '\' OR 1=1;--';
        break;
      case 5:
        this.motd = 'VaaS: Vaporware as a Service';
        break;
      case 6:
        this.motd = 'ARM-istice';
        break;
      case 7:
        this.motd = 'Crash.exe';
        break;
      case 8:
        this.motd = 'Segfault (Core Dumped)';
        break;
      case 9:
        this.motd = 'Security through Obscurity!';
        break;
      case 10:
        this.motd = 'CMMI Level -1';
        break;
      case 11:
        this.motd = 'Booting Temple OS';
        break;
      default:
        this.motd = 'Adding Buzzwords... Loading Complete!';
        break;
    }
  }

  /**
   * Login user
   */
  loginSubmit() {
    const login = new Login;
    login.name = this.loginForm.value.username ?? '';
    login.password = this.loginForm.value.password ?? '';

    this.user.login(login).subscribe({
      next: _ => {
        this.user.current().subscribe({
          next: user => {
            globals.setUser(user);
            this.router.navigateByUrl('/');
          },
          error: e => {
            this.error.handle(e);
          }
        })
      },
      error: e => {
        this.error.handle(e);
      }
    })
  }
}
