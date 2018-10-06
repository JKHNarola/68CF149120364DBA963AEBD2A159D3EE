import { Component, OnInit, OnDestroy } from "@angular/core";
import { AccountService } from "./services/account.service";
import { ApiService } from "./services/apiservice";
import { pageFadeInAnimation } from "./misc/page.animation";
import { AuthService } from "./services/app.auth.service";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";

@Component({
    selector: "app-root",
    templateUrl: "./app.component.html",
    providers: [ApiService, AccountService],
    animations: [pageFadeInAnimation]
})
export class AppComponent implements OnInit, OnDestroy {
    fullname = "";
    isUserLoggedIn: boolean = false;
    
    private currUserSetSubscription: Subscription;
    private currUserRemovedSubscription: Subscription;

    constructor(
        private authService: AuthService,
        private accountService: AccountService,
        private router: Router
    ) {
    }

    ngOnInit(): void {
        this.isUserLoggedIn = this.authService.isUserLoggedIn();
        if (this.isUserLoggedIn) {
            let currUser = this.authService.getCurrentUser();
            if (currUser != null)
                this.fullname = currUser.firstName + " " + currUser.lastName;

        }

        this.currUserSetSubscription = this.authService.onCurrUserSet.subscribe(currUser => {
            if (currUser != null) {
                this.fullname = currUser.firstName + " " + currUser.lastName;
                this.isUserLoggedIn = true;
            }
        });

        this.currUserRemovedSubscription = this.authService.onCurrUserRemoved.subscribe(isRemoved => {
            if (isRemoved) {
                this.fullname = "";
                this.isUserLoggedIn = false;
            }
        });
    }

    ngOnDestroy(): void {
        this.currUserSetSubscription.unsubscribe();
        this.currUserRemovedSubscription.unsubscribe();
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
