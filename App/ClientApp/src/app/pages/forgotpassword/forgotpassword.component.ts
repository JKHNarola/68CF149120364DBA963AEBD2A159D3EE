import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/apiservice';

@Component({
    selector: 'forgot-password',
    templateUrl: './forgotpassword.component.html',
    providers: [ApiService, AccountService]
})
export class ForgotPasswordComponent implements OnInit {
    forgotPassForm: FormGroup;
    loading = false;
    isResetPasswordEmailSent = false;

    constructor(
        private accountService: AccountService,
        private formBuilder: FormBuilder
    ) {
    }

    ngOnInit(): void {
        this.forgotPassForm = this.formBuilder.group({
            email: ['', [Validators.required, Validators.email]],
        });
    }

    get f() { return this.forgotPassForm.controls; }

    onResetPasswordRequest() {
        this.loading = true;

        if (this.forgotPassForm.invalid) {
            this.loading = false;
            return;
        }

        let email = this.forgotPassForm.value.email;

        this.accountService.requestResetPassword(email).subscribe(
            res => {
                if (res.status === 1) {
                    this.isResetPasswordEmailSent = true;
                }
                this.loading = false;
            },
            err => {
                this.loading = false;
            });
    }
}
