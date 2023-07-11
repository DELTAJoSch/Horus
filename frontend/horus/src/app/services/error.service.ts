import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {

  constructor(private notification: ToastrService,
    private router: Router) { }

  handle(error: any) {
    console.error(error);

    if(error.status === 401){
      this.router.navigateByUrl('/login');
      return;
    }

    if(error.status === 500){
      this.notification.error('An unexpected error has occured!');
      return;
    }

    if(error.error !== undefined && error.error !== null && typeof error.error === 'string'){
      this.notification.error(error.error);
      return;
    }

    this.notification.error('An unexpected error has occured!');
  }
}
