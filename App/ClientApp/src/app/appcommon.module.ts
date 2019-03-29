import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { ToastrModule } from 'ngx-toastr';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerModule } from 'ngx-spinner';
import { AppPaginationComponent } from './components/pagination/app.pagination';
import { AppSortComponent } from './components/sort/app.sort';
import { HttpClientModule } from '@angular/common/http';
import { AppLoaderComponent } from './components/loader/app.loader';

@NgModule({
    declarations: [
        AppPaginationComponent,
        AppSortComponent,
        AppLoaderComponent
    ],
    imports: [
        BrowserModule,
        FormsModule,
        ReactiveFormsModule,
        BrowserAnimationsModule,
        HttpClientModule,
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
        NgbModule,
        NgxSpinnerModule
    ],
    exports: [
        AppPaginationComponent,
        AppSortComponent,
        AppLoaderComponent,
        BrowserModule,
        FormsModule,
        ReactiveFormsModule,
        BrowserAnimationsModule,
        HttpClientModule,
        ToastrModule,
        NgbModule,
        NgxSpinnerModule
    ]
})
export class AppCommonModule { }
