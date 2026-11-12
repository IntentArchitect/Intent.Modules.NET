import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-customer-summary-card',
  standalone: true,
  templateUrl: 'customer-summary-card.component.html'
})
export class CustomerSummaryCardComponent {
  @Input() count: number | null = null;
}
