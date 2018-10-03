import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AccountService } from '../../services/account.service';
import { ApiService } from '../../services/apiservice';
import { AuthService } from '../../services/app.auth.service';
import { ToastrService } from 'ngx-toastr';
import { Router, ActivatedRoute } from '@angular/router';
import { ResetPasswordModel } from '../../models/account/resetpassword.model';
import { pageSlideUpAnimation } from '../../misc/page.animation';

@Component({
    selector: 'reset-password',
    templateUrl: './resetpassword.component.html',
    providers: [ApiService, AccountService],
    animations: [pageSlideUpAnimation]
})
export class ResetPasswordComponent implements OnInit {
    resetpasswordForm: FormGroup;
    loading = false;
    email: string;
    code: string;

    constructor(
        private route: ActivatedRoute,
        private accountService: AccountService,
        private authService: AuthService,
        private formBuilder: FormBuilder,
        private toastrService: ToastrService,
        private router: Router) {
        this.email = this.route.snapshot.queryParams.email;
        this.code = this.route.snapshot.queryParams.code;

        if (!this.email || !this.code) {
            this.router.navigate(['/home']);
        }
    }

    ngOnInit(): void {
        this.resetpasswordForm = this.formBuilder.group({
            newPassword: ['', [Validators.required, Validators.minLength(8)]],
        });
    }

    get f() { return this.resetpasswordForm.controls; }

    onResetPassword() {
        this.loading = true;

        if (this.resetpasswordForm.invalid) {
            this.loading = false;
            return;
        }

        let model = new ResetPasswordModel();
        model.email = this.email;
        model.code = this.code;
        model.newPassword = this.resetpasswordForm.value.newPassword;

        this.accountService.resetPassword(model).subscribe(
            res => {
                if (res.status === 1) {
                    this.authService.setCurrentUser(res.data);
                    this.toastrService.success("Your password is successfully reset.");
                    this.router.navigate(['/home']);
                }
                else if (res.status === 2) {
                    this.toastrService.error("The code for resetting password is seems to be invalid or exipred.");
                }
                this.resetpasswordForm.reset();
                this.loading = false;
            },
            err => {
                this.loading = false;
            });
    }
}
