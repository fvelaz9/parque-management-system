import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HistorialCanje } from './historial-canje';

describe('HistorialCanje', () => {
  let component: HistorialCanje;
  let fixture: ComponentFixture<HistorialCanje>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HistorialCanje]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HistorialCanje);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
