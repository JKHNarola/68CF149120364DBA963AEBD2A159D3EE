import { Component, OnInit, HostBinding } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AccountService } from '../../services/account.service';
import { ApiService } from '../../services/apiservice';
import { RegisterModel } from '../../models/account/register.model';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { routerTransition } from '../../misc/page.animation';

@Component({
    selector: 'register',
    templateUrl: './register.component.html',
    providers: [ApiService, AccountService],
    animations: [routerTransition]

})
export class RegisterComponent implements OnInit {
    @HostBinding("@routerTransition")
    routeAnimation = true;
    @HostBinding("style.display")
    display = "block";

    registerForm: FormGroup;
    loading = false;
    isConfirmEmailSent = false;

    constructor(
        private accountService: AccountService,
        private formBuilder: FormBuilder,
        private toastrService: ToastrService,
        private router: Router) {
    }

    ngOnInit(): void {
        this.registerForm = this.formBuilder.group({
            firstName: ['', [Validators.required, Validators.maxLength(60)]],
            lastName: ['', [Validators.required, Validators.maxLength(60)]],
            email: ['', [Validators.required, Validators.email]]
        });
    }

    get f() { return this.registerForm.controls; }

    onRegister() {
        this.loading = true;

        if (this.registerForm.invalid) {
            this.loading = false;
            return;
        }

        let model = new RegisterModel();
        model.email = this.registerForm.value.email;
        model.firstName = this.registerForm.value.firstName;
        model.lastName = this.registerForm.value.lastName;

        // model = new RegisterModel();

        this.accountService.register(model).subscribe(
            res => {
                switch (res.status) {
                    case 1:
                        this.isConfirmEmailSent = true;
                        this.registerForm.reset();
                        break;
                    case -3:
                        this.toastrService.error("The email you have entered, is already exists. Please try another email.");
                        break;
                    case -4:
                        this.toastrService.error("User was successfully created but we failed to sent email. Please try again.");
                        break;
                    case -6:
                        this.toastrService.error("User is successfully created but we failed to sent email. Please contact administrator for help.");
                        break;
                    case -7:
                        this.toastrService.error("User is successfully created but we failed to set role. Please contact administrator for help.");
                        break;
                    case -8:
                        this.toastrService.error("User was successfully created but we failed to set role. Please try again.");
                        break;
                    default:
                        this.toastrService.error("Some error occured on server during registration process. Please try again.");
                        break;
                }

                this.loading = false;
            },
            err => {
                this.loading = false;
            });
    }
}
