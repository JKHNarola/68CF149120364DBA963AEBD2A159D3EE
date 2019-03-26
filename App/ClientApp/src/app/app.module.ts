import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { routes, AuthGuardService } from './app.router';
import { AuthService } from './services/app.auth.service';
import { RequestInterceptor } from './misc/httprequest.intereceptor';
import { ResponseInterceptor } from './misc/httpresponse.interceptor';

import { ToastrModule } from 'ngx-toastr';
import { FusionChartsModule } from 'angular-fusioncharts';
import FusionCharts from 'fusioncharts/core';
import Column2D from 'fusioncharts/viz/column2d';
import Doughnut2d from 'fusioncharts/viz/doughnut2d';
import MsLine from 'fusioncharts/viz/msline';
import Bubble from 'fusioncharts/viz/bubble';
import * as FusionTheme from 'fusioncharts/themes/fusioncharts.theme.fusion';
import * as CandyTheme from 'fusioncharts/themes/fusioncharts.theme.candy';
import * as CarbonTheme from 'fusioncharts/themes/fusioncharts.theme.carbon';
import * as GammelTheme from 'fusioncharts/themes/fusioncharts.theme.gammel';
import * as OceanTheme from 'fusioncharts/themes/fusioncharts.theme.ocean';
import * as ZuneTheme from 'fusioncharts/themes/fusioncharts.theme.zune';
import * as FintTheme from 'fusioncharts/themes/fusioncharts.theme.fint';
FusionChartsModule.fcRoot(
    FusionCharts,
    Column2D,
    Doughnut2d,
    MsLine,
    Bubble,
    FusionTheme,
    CandyTheme,
    GammelTheme,
    CarbonTheme,
    OceanTheme,
    ZuneTheme,
    FintTheme);
import { PaginationModule} from 'ngx-bootstrap/pagination';

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
import { AppPaginationComponent } from './components/app.pagination';

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
        ChangePasswordComponent,
        AppPaginationComponent 
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
        }),
        FusionChartsModule,
        PaginationModule.forRoot()
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
