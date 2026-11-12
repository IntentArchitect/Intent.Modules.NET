//@IntentMerge()
import { IntentIgnoreBody, IntentMerge, IntentIgnore } from './../../../intent/intent.decorators';
import { CustomersService } from './../../../service-proxies/customers/customers-service';
import { CustomerSummaryCardComponent } from './../customer-summary-card/customer-summary-card.component';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { CommonModule } from '@angular/common';

@IntentMerge()
@Component({
  selector: 'app-customer-list-page',
  standalone: true,
  templateUrl: 'customer-list-page.component.html',
  styleUrls: ['customer-list-page.component.scss'],
  imports: [CommonModule, CustomerSummaryCardComponent]
})
export class CustomerListPageComponent implements OnInit {
  isLoading = false;
  customersModels: any = null;

  //@IntentMerge()
  constructor(private router: Router, private readonly customersService: CustomersService) {
  }

  @IntentMerge()
  ngOnInit(): void {
    this.loadCustomers(1, 10, 'name');
  }

  @IntentMerge()
  loadCustomers(pageNo: number, pageSize: number, orderBy: string): void {
    this.isLoading = true;
    this.customersService.getCustomers({
      pageNo: pageNo,
      pageSize: pageSize,
      orderBy: orderBy
    })
      .pipe(finalize(() => { this.isLoading = false; }))
      .subscribe({
        next: (data) => {
          this.customersModels = data;
        }
      });
  }

  navigateToCustomerAddPage(): void {
    this.router.navigate(['/customer', 'add']);
  }

  navigateToCustomerEditPage(customerId: string): void {
    this.router.navigate(['/customer', 'edit', customerId]);
  }
}
