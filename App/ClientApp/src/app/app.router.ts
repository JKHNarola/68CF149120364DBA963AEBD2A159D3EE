import { Routes, Router } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { RegisterComponent } from './pages/register/register.component';
import { Injectable } from "@angular/core";
import { CanActivate } from "@angular/router";
import { AuthService } from "./services/app.auth.service";
import { SetPasswordComponent } from './pages/setpassword/setpassword.component';
import { ConfirmEmailComponent } from './pages/confirmemail/confirmemail.component';
import { Page1Component } from './pages/page1/page1.component';
import { Page2Component } from './pages/page2/page2.component';
import { Page3Component } from './pages/page3/page3.component';
import { Page4Component } from './pages/page4/page4.component';
import { ForgotPasswordComponent } from './pages/forgotpassword/forgotpassword.component';
import { ResetPasswordComponent } from './pages/resetpassword/resetpassword.component';
import { ChangePasswordComponent } from './pages/changepassword/changepassword.component';

@Injectable()
export class AuthGuardService implements CanActivate {
    constructor(
        private authService: AuthService,
        private router: Router
    ) { };

    canActivate() {
        if (this.authService.isUserLoggedIn() && !this.authService.isTokenExpired()) {
            return true;
        } else {
            this.router.navigate(['/login']);
            return false;
        }
    }
}

export const routes: Routes = [
    { path: '', redirectTo: "/home", pathMatch: "full" },
    { path: 'login', component: LoginComponent },
    {
        path: 'home',
        component: HomeComponent,
        canActivate: [AuthGuardService],
        children: [
            {
                path: 'page1',
                component: Page1Component
            },
            {
                path: 'page2',
                component: Page2Component
            },
            {
                path: 'page3',
                component: Page3Component
            },
            {
                path: 'page4',
                component: Page4Component
            },
            {
                path: 'changepassword',
                component: ChangePasswordComponent
            },
        ]
    },
    { path: 'register', component: RegisterComponent },
    { path: 'confirmemail', component: ConfirmEmailComponent },
    { path: 'setpassword', component: SetPasswordComponent },
    { path: 'forgotpassword', component: ForgotPasswordComponent },
    { path: 'resetpassword', component: ResetPasswordComponent },
    { path: '*', redirectTo: "/home" },
    { path: '**', redirectTo: "/home" },
];
