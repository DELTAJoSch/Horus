import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, HostListener, OnInit } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  animations: [
    trigger('visibleHidden', [
      state(
        'visible',
        style({
          opacity: 1
        })
      ),
      state(
        'hidden, void',
        style({
          opacity: 0
        })
      ),
      transition('* => visible', animate('500ms ease-in-out')),
      transition('* => hidden, * => void', animate('500ms ease-in-out'))
    ])
  ]
})
export class HeaderComponent implements OnInit {
  mobile = false;
  mobileMenu = false;

  /**
   * Listens to window resize events
   */
  @HostListener('window:resize')
  onResize(){
    if(window.innerWidth < 1000){
      this.mobile = true;
    }else{
      this.mobile = false;
      this.mobileMenu = false;
    }
  }

  /**
   * Initializes this component
   */
  ngOnInit(){
    if(window.innerWidth < 1000){
      this.mobile = true;
    }else{
      this.mobile = false;
      this.mobileMenu = false;
    }
  }

  /**
   * toggles the mobile menu
   */
  mobileMenuToggle(){
    this.mobileMenu = !this.mobileMenu;
  }
}
