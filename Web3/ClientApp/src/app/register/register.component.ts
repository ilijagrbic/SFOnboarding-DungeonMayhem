import { Component, EventEmitter, Output } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  username: string = '';
  password: string = '';
  @Output() registerResponse = new EventEmitter<any>();

  constructor(private http: HttpClient) {}

  onSubmit() {
    const body = { username: this.username, password: this.password };

    this.http.post(environment.backendHost + 'api/User/register', body)
      .subscribe(response => {
        var jsonResponse:any = response as JSON;
        console.log('User registered successfully', jsonResponse);
        this.registerResponse.emit(jsonResponse.username);
      }, error => {
        console.error('Error registering user', error);
      });
  }
}
