import { NgModule, ErrorHandler } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

import { routes, AuthGuardService } from './app.router';
import { AuthService } from './services/app.auth.service';
import { RequestInterceptor } from './misc/httprequest.intereceptor';
import { ResponseInterceptor } from './misc/httpresponse.interceptor';

import { AppComponent } from './app.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { SetPasswordComponent } from './pages/setpassword/setpassword.component';
import { ConfirmEmailComponent } from './pages/confirmemail/confirmemail.component';
import { Page1Component } from './pages/page1/page1.component';
import { Page2Component } from './pages/page2/page2.component';
import { Page3Component } from './pages/page3/page3.component';
import { Page4Component } from './pages/page4/page4.component';
import { ForgotPasswordComponent } from './pages/forgotpassword/forgotpassword.component';
import { ResetPasswordComponent } from './pages/resetpassword/resetpassword.component';
import { ChangePasswordComponent } from './pages/changepassword/changepassword.component';
import { GlobalErrorHandler } from './misc/global.errorhandler';
import { FusionChartModule } from './fusionchart.module';
import { AppCommonModule } from './appcommon.module';
import { RouterModule } from '@angular/router';

@NgModule({
    declarations: [
        AppComponent,
        LoginComponent,
        RegisterComponent,
        SetPasswordComponent,
        ConfirmEmailComponent,
        Page1Component,
        Page2Component,
        Page3Component,
        Page4Component,
        ForgotPasswordComponent,
        ResetPasswordComponent,
        ChangePasswordComponent
    ],
    imports: [
        AppCommonModule,
        FusionChartModule,
        RouterModule.forRoot(routes)
    ],
    providers: [
        AuthService,
        AuthGuardService,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: RequestInterceptor,
            multi: true,
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: ResponseInterceptor,
            multi: true,
        },
        {
            provide: ErrorHandler,
            useClass: GlobalErrorHandler
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
