import { Component } from '@angular/core';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;
  componentShown: string = 'login';
  userName: string = "";

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout() {
    this.userName = "";
    this.componentShown = 'login';
  }

  show(component: any) {
    this.componentShown = component;
  }

  handleLoginResponse(response: any) {
    console.log('User logged in: ' + response);
    this.userName = response;
    this.componentShown = 'game';
  }
}
