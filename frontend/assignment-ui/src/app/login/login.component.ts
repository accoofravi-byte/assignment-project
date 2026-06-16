import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})

export class LoginComponent {
  model = { 
    username: '',
    password: ''
  };

  errorMessage = '';


  constructor(
    private auth: AuthService, 
    private router: Router,
    private cdr: ChangeDetectorRef,
  ) {}

  login() {
    console.log('----Login starts----');
    console.log('Model:', this.model);

    const payload = {
      username: btoa(this.model.username),
      password: btoa(this.model.password)
    };

    console.log('Base64 Encoded Payload:', payload);

    this.auth.login(payload)
    .subscribe({
      next: (response: any) => {
         this.errorMessage = '';
        console.log('Login Success', response);
        console.log('Response', response);

        this.auth.saveToken(response.token);
        console.log('Token saved:', this.auth.getToken());

        this.router.navigate(['/products'])
        .then(result=>{
          console.log('Navigation result:', result);  
        })
        .catch(err=>{
          console.error('Navigation error:', err);
        });
      },

      error: (err) => {
        this.showError('Invalid username or password');
        console.error('LOGIN ERROR', err);

        if(err.error){
          console.error('Error body:', err.error);
        }
        
        console.log('Status:', err.status)
        console.log('Status Text:', err.statusText)
        console.log('Url:', err.url)
      }
    });
  }

  showError(message: string){
    this.errorMessage = message;
    setTimeout(() => {
      this.errorMessage = '';
      this.cdr.detectChanges();
    }, 3000);
  }
}
