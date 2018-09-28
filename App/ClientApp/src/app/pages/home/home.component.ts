import { Component } from '@angular/core';
import { AuthService } from '../../services/app.auth.service';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
})
export class HomeComponent {
    fullName: string = "";
    constructor(private authService: AuthService) {
        let currUser = this.authService.getCurrentUser();
        this.fullName = currUser.firstName + " " + currUser.lastName;
    }
}
