import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UnownedCardsListComponent } from './unowned-cards-list.component';

describe('UnownedCardsListComponent', () => {
  let component: UnownedCardsListComponent;
  let fixture: ComponentFixture<UnownedCardsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UnownedCardsListComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UnownedCardsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
