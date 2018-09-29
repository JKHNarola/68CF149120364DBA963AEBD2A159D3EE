import { Component } from '@angular/core';
import { AuthService } from '../../services/app.auth.service';
import { ApiService } from '../../services/apiservice';
import { AccountService } from '../../services/account.service';
import { Router } from '@angular/router';
import { pageFadeInAnimation } from '../../misc/page.animation';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    providers: [ApiService, AccountService],
    animations: [pageFadeInAnimation]
})
export class HomeComponent {
    fullName: string = "";
    constructor(
        private authService: AuthService,
        private accountService: AccountService,
        private router: Router
    ) {
        let currUser = this.authService.getCurrentUser();
        this.fullName = currUser.firstName + " " + currUser.lastName;
    }

    onLogout() {
        this.accountService.logout().subscribe(res => {
            if (res.status === 1) {
                this.authService.removeCurrentUser();
                this.router.navigate(['/login']);
            }
        });
    }
}
