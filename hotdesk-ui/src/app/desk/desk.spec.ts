import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Desk } from './desk';

describe('Desk', () => {
  let component: Desk;
  let fixture: ComponentFixture<Desk>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Desk]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Desk);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
