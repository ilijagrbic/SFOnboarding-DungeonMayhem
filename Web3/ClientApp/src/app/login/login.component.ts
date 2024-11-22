import { Component, EventEmitter, Output } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  @Output() loginResponse = new EventEmitter<any>();

  constructor(private http: HttpClient) { }

  onSubmit() {
    const user = { username: this.username, password: this.password };
    this.http.put(environment.backendHost + 'api/User/login', user)
     .subscribe(response => {
       console.log('User logged in successfully', response);
       this.loginResponse.emit(user.username);
     }, error => {
       console.error('Error logging in user', error);
     });
  }
}
