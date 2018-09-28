import { Component } from '@angular/core';
import { AuthService } from '../../services/app.auth.service';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
})
export class HomeComponent {
    constructor(private authService: AuthService) {
        this.authService.getToken();
    }
}
