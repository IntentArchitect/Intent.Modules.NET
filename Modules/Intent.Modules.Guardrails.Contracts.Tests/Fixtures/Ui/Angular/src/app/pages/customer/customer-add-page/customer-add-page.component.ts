//@IntentMerge()
import { IntentIgnoreBody, IntentMerge, IntentIgnore } from './../../../intent/intent.decorators';
import { CustomersService } from './../../../service-proxies/customers/customers-service';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

@IntentMerge()
@Component({
  selector: 'app-customer-add-page',
  standalone: true,
  templateUrl: 'customer-add-page.component.html'
})
export class CustomerAddPageComponent {
  isLoading = false;
  model = { name: '', surname: '', email: '' };

  //@IntentMerge()
  constructor(private router: Router, private readonly customersService: CustomersService) {
  }

  @IntentMerge()
  createCustomer(): void {
    this.isLoading = true;
    this.customersService.createCustomer({
      name: this.model.name,
      surname: this.model.surname,
      email: this.model.email
    })
      .pipe(finalize(() => { this.isLoading = false; }))
      .subscribe({
        next: () => {
          this.navigateToCustomerListPage();
        }
      });
  }

  navigateToCustomerListPage(): void {
    this.router.navigate(['/customer-list']);
  }
}
