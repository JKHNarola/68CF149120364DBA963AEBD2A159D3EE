import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { ToastrModule } from 'ngx-toastr';
import { routes, AuthGuardService } from './app.router';
import { AuthService } from './services/app.auth.service';
import { JwtInterceptor } from './misc/JwtInterceptor';
import { ErrorInterceptor } from './misc/errorinterceptor';

import { AppComponent } from './app.component';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { RegisterComponent } from './pages/register/register.component';
import { SetPasswordComponent } from './pages/setpassword/setpassword.component';
import { ConfirmEmailComponent } from './pages/confirmemail/confirmemail.component';
import { Page1Component } from './pages/page1/page1.component';
import { Page2Component } from './pages/page2/page2.component';
import { Page3Component } from './pages/page3/page3.component';
import { Page4Component } from './pages/page4/page4.component';
import { ForgotPasswordComponent } from './pages/forgotpassword/forgotpassword.component';
import { ResetPasswordComponent } from './pages/resetpassword/resetpassword.component';

@NgModule({
    declarations: [
        AppComponent,
        LoginComponent,
        HomeComponent,
        RegisterComponent,
        SetPasswordComponent,
        ConfirmEmailComponent,
        Page1Component,
        Page2Component,
        Page3Component,
        Page4Component,
        ForgotPasswordComponent,
        ResetPasswordComponent
    ],
    imports: [
        BrowserModule,
        RouterModule.forRoot(routes),
        ReactiveFormsModule,
        HttpClientModule,
        BrowserAnimationsModule,
        ToastrModule.forRoot({
            timeOut: 5000,
            extendedTimeOut: 3000,
            enableHtml: true,
            progressBar: true,
            positionClass: 'toast-top-right',
            preventDuplicates: true,
            resetTimeoutOnDuplicate: true,
            closeButton: true
        })
    ],
    providers: [
        AuthService,
        AuthGuardService,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: JwtInterceptor,
            multi: true,
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: ErrorInterceptor,
            multi: true,
        },
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
